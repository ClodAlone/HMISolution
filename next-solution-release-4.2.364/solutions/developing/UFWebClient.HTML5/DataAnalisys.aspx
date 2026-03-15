<%@ Page Title="DataAnalisys" Language="C#" MasterPageFile="~/ServerStuff/ServerStuff.master" AutoEventWireup="true" CodeBehind="DataAnalisys.aspx.cs" Inherits="UFWebClient.HTML5.DataAnalisys" %>

<%@ Register assembly="DevExpress.Web.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server">
    DataAnalisys
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">

<% if (HttpContext.Current.IsDebuggingEnabled) { %>
    <script type="text/javascript" src="Scripts/jquery-2.2.4.js"></script>
    <script type="text/javascript" src="Scripts/jquery.signalR-2.2.1.js"></script>
<% } else { %>
    <script type="text/javascript" src="Scripts/jquery-2.2.4.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.signalR-2.2.1.min.js"></script>
<% } %>
    <script type="text/javascript" src="SignalR/Hubs"></script>
    
    <script type="text/javascript">

        window.onload = init;

        window.addEventListener("resize", OnWindowResize, false);

        function OnWindowResize(e) {
            splitter.SetHeight(window.innerHeight);
       }

        var dataHub = $.connection.dataHub;

        $.connection.hub.connectionSlow(function () {
            console.log("Slow connection detected");
            LoadingPanel.SetText("Slow connection detected");
            LoadingPanel.Show();

            clearTimeout(timerSlowConnection);
            timerSlowConnection = setTimeout("slowConnectionTimeout()", 2000);
        });

        function slowConnectionTimeout() {
            clearTimeout(timerSlowConnection);
            LoadingPanel.Hide();
        }

        $.connection.hub.error(function (error) {
            console.log(error.message);
        });

        $.connection.hub.stateChanged(function (state) {
            if (state.newState == 0) {
                console.log("STATE: Connecting...");
                LoadingPanel.SetText("Connecting");
                LoadingPanel.Show();
            }
            else if (state.newState == 1) {
                console.log("STATE: Connected...");
                LoadingPanel.SetText("Connected");
                LoadingPanel.Hide();
            }
            else if (state.newState == 2) {
                console.log("STATE: Reconnecting...");
                LoadingPanel.SetText("Reconnecting...");
                LoadingPanel.Show();
            }
            else if (state.newState == 3) {
                console.log("STATE: Disconnected...");
                LoadingPanel.SetText("Disconnected...");
                LoadingPanel.Show();
            }
            else {
                console.log("STATE: Unknown");
                LoadingPanel.SetText("Unknown connection status");
                LoadingPanel.Show();
            }
        });

        function init() {

            LoadingPanel.Show();

            var deferredStart = $.connection.hub.start();
            deferredStart.done(function () {
                console.log("My connection id is " + $.connection.hub.id);
                var id = $.connection.hub.id;

                LoadingPanel.Hide();

                chart.render();
            });
        }

        function onSuccessGetData(data) {
            
            if (data.Error != null)
            {
                LoadingPanel.Hide();
                writetostatus(data.Error);
                return;
            }

            var dataSource = [];
            var series = [];

            for (var i = 0; i < data.DateTimes.length; i++) 
            {
                var item = {};
                item.arg = data.DateTimes[i];

                for(var k = 0; k < data.Names.length; ++k)
                {
                    item[data.Names[k]] = data.Values[k][i];
                }

                dataSource.push(item);
            }

            for (var k = 0; k < data.Names.length; ++k) {

                var serie = {};
                serie.argumentField = "arg";
                serie.valueField = data.Names[k];
                serie.name = data.Names[k];
                series.push(serie);
            }

            var title = {
                text: data.Name
            };

            chart.option("useAggregation", dataSource.length > 5000);
            chart.option("title", title);
            chart.option("series", series);
            chart.option("dataSource", dataSource);

            LoadingPanel.Hide();

            deStart.SetDate(new Date(data.Start));
            deEnd.SetDate(new Date(data.End));
        }

        function onFailureGetData() {
            LoadingPanel.Hide();
            writetostatus("An error occured fetching the data from the server");
        }

        function writetostatus(input) {
            window.status = input
            alert(input);
        }

        function UpdateInfo() {
            var daysTotal = deEnd.GetRangeDayCount();
            tbInfo.SetText(daysTotal !== -1 ? daysTotal + ' days' : '');
        }

        function btnFetch_Click() {

            var selectedSource = listBox.GetSelectedItem().text;

            LoadingPanel.SetText('Loading Data');
            LoadingPanel.Show();

            dataHub.server.getDataSource(selectedSource, deStart.GetDate(), deEnd.GetDate()).
                    done(onSuccessGetData).
                    fail(onFailureGetData);
        }
    </script>

    <script type="text/javascript" src="Scripts/knockout-3.4.0.js"></script>    <script type="text/javascript" src="Scripts/globalize.min.js"></script>    <script type="text/javascript" src="Scripts/dx.all.js"></script>    <script type="text/javascript" src="Scripts/jszip.min.js"></script>    <script type="text/javascript" src="Scripts/underscore-1.5.1.min.js"></script>
    <script id="jsCode">

        var chart;

        $(function () {
            var sampleData = [
                { arg: 10, y1: -12, y2: 10, y3: 32 },
                { arg: 20, y1: -32, y2: 30, y3: 12 },
                { arg: 40, y1: -20, y2: 20, y3: 30 },
                { arg: 50, y1: -39, y2: 50, y3: 19 },
                { arg: 60, y1: -10, y2: 10, y3: 15 },
                { arg: 75, y1: 10, y2: 10, y3: 15 },
                { arg: 80, y1: 30, y2: 50, y3: 13 },
                { arg: 90, y1: 40, y2: 50, y3: 14 },
                { arg: 100, y1: 50, y2: 90, y3: 90 },
                { arg: 105, y1: 40, y2: 175, y3: 120 },
                { arg: 110, y1: -12, y2: 10, y3: 32 },
                { arg: 120, y1: -32, y2: 30, y3: 12 },
                { arg: 130, y1: -20, y2: 20, y3: 30 },
                { arg: 140, y1: -12, y2: 10, y3: 32 },
                { arg: 150, y1: -32, y2: 30, y3: 12 },
                { arg: 160, y1: -20, y2: 20, y3: 30 },
                { arg: 170, y1: -39, y2: 50, y3: 19 },
                { arg: 180, y1: -10, y2: 10, y3: 15 },
                { arg: 185, y1: 10, y2: 10, y3: 15 },
                { arg: 190, y1: 30, y2: 100, y3: 13 },
                { arg: 200, y1: 40, y2: 110, y3: 14 },
                { arg: 210, y1: 50, y2: 90, y3: 90 },
                { arg: 220, y1: 40, y2: 95, y3: 120 },
                { arg: 230, y1: -12, y2: 10, y3: 32 },
                { arg: 240, y1: -32, y2: 30, y3: 12 },
                { arg: 255, y1: -20, y2: 20, y3: 30 },
                { arg: 270, y1: -12, y2: 10, y3: 32 },
                { arg: 280, y1: -32, y2: 30, y3: 12 },
                { arg: 290, y1: -20, y2: 20, y3: 30 },
                { arg: 295, y1: -39, y2: 50, y3: 19 },
                { arg: 300, y1: -10, y2: 10, y3: 15 },
                { arg: 310, y1: 10, y2: 10, y3: 15 },
                { arg: 320, y1: 30, y2: 100, y3: 13 },
                { arg: 330, y1: 40, y2: 110, y3: 14 },
                { arg: 340, y1: 50, y2: 90, y3: 90 },
                { arg: 350, y1: 40, y2: 95, y3: 120 },
                { arg: 360, y1: -12, y2: 10, y3: 32 },
                { arg: 367, y1: -32, y2: 30, y3: 12 },
                { arg: 370, y1: -20, y2: 20, y3: 30 },
                { arg: 380, y1: -12, y2: 10, y3: 32 },
                { arg: 390, y1: -32, y2: 30, y3: 12 },
                { arg: 400, y1: -20, y2: 20, y3: 30 },
                { arg: 410, y1: -39, y2: 50, y3: 19 },
                { arg: 420, y1: -10, y2: 10, y3: 15 },
                { arg: 430, y1: 10, y2: 10, y3: 15 },
                { arg: 440, y1: 30, y2: 100, y3: 13 },
                { arg: 450, y1: 40, y2: 110, y3: 14 },
                { arg: 460, y1: 50, y2: 90, y3: 90 },
                { arg: 470, y1: 40, y2: 95, y3: 120 },
                { arg: 480, y1: -12, y2: 10, y3: 32 },
                { arg: 490, y1: -32, y2: 30, y3: 12 },
                { arg: 500, y1: -20, y2: 20, y3: 30 },
                { arg: 510, y1: -12, y2: 10, y3: 32 },
                { arg: 520, y1: -32, y2: 30, y3: 12 },
                { arg: 530, y1: -20, y2: 20, y3: 30 },
                { arg: 540, y1: -39, y2: 50, y3: 19 },
                { arg: 550, y1: -10, y2: 10, y3: 15 },
                { arg: 555, y1: 10, y2: 10, y3: 15 },
                { arg: 560, y1: 30, y2: 100, y3: 13 },
                { arg: 570, y1: 40, y2: 110, y3: 14 },
                { arg: 580, y1: 50, y2: 90, y3: 90 },
                { arg: 590, y1: 40, y2: 95, y3: 12 },
                { arg: 600, y1: -12, y2: 10, y3: 32 },
                { arg: 610, y1: -32, y2: 30, y3: 12 },
                { arg: 620, y1: -20, y2: 20, y3: 30 },
                { arg: 630, y1: -12, y2: 10, y3: 32 },
                { arg: 640, y1: -32, y2: 30, y3: 12 },
                { arg: 650, y1: -20, y2: 20, y3: 30 },
                { arg: 660, y1: -39, y2: 50, y3: 19 },
                { arg: 670, y1: -10, y2: 10, y3: 15 },
                { arg: 680, y1: 10, y2: 10, y3: 15 },
                { arg: 690, y1: 30, y2: 100, y3: 13 },
                { arg: 700, y1: 40, y2: 110, y3: 14 },
                { arg: 710, y1: 50, y2: 90, y3: 90 },
                { arg: 720, y1: 40, y2: 95, y3: 120 },
                { arg: 730, y1: 20, y2: 190, y3: 130 },
                { arg: 740, y1: -32, y2: 30, y3: 12 },
                { arg: 750, y1: -20, y2: 20, y3: 30 },
                { arg: 760, y1: -12, y2: 10, y3: 32 },
                { arg: 770, y1: -32, y2: 30, y3: 12 },
                { arg: 780, y1: -20, y2: 20, y3: 30 },
                { arg: 790, y1: -39, y2: 50, y3: 19 },
                { arg: 800, y1: -10, y2: 10, y3: 15 },
                { arg: 810, y1: 10, y2: 10, y3: 15 },
                { arg: 820, y1: 30, y2: 100, y3: 13 },
                { arg: 830, y1: 40, y2: 110, y3: 14 },
                { arg: 840, y1: 50, y2: 90, y3: 90 },
                { arg: 850, y1: 40, y2: 95, y3: 120 },
                { arg: 860, y1: -12, y2: 10, y3: 32 },
                { arg: 870, y1: -32, y2: 30, y3: 12 },
                { arg: 880, y1: -20, y2: 20, y3: 30 }
            ];

            chart = $("#chartContainer").dxChart({
                dataSource: sampleData,

                useAggregation: false,

                onLegendClick: function (e) {
                    var series = e.target;
                    series.isVisible() ? series.hide() : series.show();
                },
                commonSeriesSettings: {
                    argumentField: "arg",
                    type: "spline",
                    point: {
                        hoverMode: "allArgumentPoints"
                    }
                },
                argumentAxis: {
                    valueMarginsEnabled: false,
                    discreteAxisDivisionMode: "crossLabels",
                    grid: {
                        visible: true
                    }
                },
                crosshair: {
                    enabled: true,
                    color: "#949494",
                    width: 3,
                    dashStyle: "dot",
                    label: {
                        visible: true,
                        backgroundColor: "#949494",
                        font: {
                            color: "#fff",
                            size: 12,
                        }
                    }
                },
                scrollBar: {
                    visible: true
                },
                scrollingMode: "all",
                zoomingMode: "all",
                series: [{
                    argumentField: "arg",
                    valueField: "y1"
                }, {
                    argumentField: "arg",
                    valueField: "y2"
                }],
                legend: {
                    verticalAlignment: "bottom",
                    horizontalAlignment: "center",
                    itemTextPosition: "bottom",
                    equalColumnWidth: true
                },
                title: {
                    text: "Sample Data"
                },
                tooltip: {
                    enabled: true,
                    customizeTooltip: function () {
                        return this.valueText;
                    }
                }
            }).dxChart("instance");
        });
    </script>

    <dx:ASPxLoadingPanel ID="ASPxLoadingPanel1" runat="server" Modal="true" HorizontalAlign="Center"        
            ClientInstanceName="LoadingPanel" Text="Please, wait...">               
    </dx:ASPxLoadingPanel>

    <dx:ASPxSplitter ID="ASPxSplitter1" runat="server" Width="100%" Height="1024px" ResizingMode="Live" ClientInstanceName="splitter">
        <panes>
            <dx:SplitterPane Size="20%" Name="listBoxContainer" ShowCollapseBackwardButton="True">
                <ContentCollection>
                    <dx:SplitterContentControl ID="SplitterContentControl1" runat="server" SupportsDisabledAttribute="True">
                        <div id="listBoxContainer" style="height: 100%; width: 100%;">
						    <dx:ASPxListBox ID="ASPxListBox1" runat="server" Height="100%" Width="100%" AutoPostBack="False" ClientInstanceName="listBox">
                                <ClientSideEvents SelectedIndexChanged="function(s, e) {
	                                    var selectedSource = s.GetSelectedItem().text;

                                        LoadingPanel.SetText('Loading Data');
                                        LoadingPanel.Show();

                                        dataHub.server.getDataSource(selectedSource, new Date(0), new Date(0)).
                                                done(onSuccessGetData).
                                                fail(onFailureGetData);
                                    }" 
                                    />						
						    </dx:ASPxListBox>
                        </div>
                    </dx:SplitterContentControl>
                </ContentCollection>
            </dx:SplitterPane>
            <dx:SplitterPane ShowCollapseBackwardButton="True" Size="80%">
                <Panes>
                    <dx:SplitterPane Name="chartPaneContainer" Size="70%" MinSize="100px">
                        <ContentCollection>
                            <dx:SplitterContentControl ID="SplitterContentControl2" runat="server" SupportsDisabledAttribute="True">
                                <div id="chartContainer" style="width: 98%; height: 100%;"></div>                            </dx:SplitterContentControl>
                        </ContentCollection>
                    </dx:SplitterPane>
                    <dx:SplitterPane Name="editorsContainer" MinSize="100px" ShowCollapseForwardButton="True">
                        <ContentCollection>
                            <dx:SplitterContentControl runat="server">

                                <dx:ASPxFormLayout ID="flDateRangePicker" runat="server" ColCount="2" RequiredMarkDisplayMode="None">
                                        <SettingsItemCaptions Location="Top"></SettingsItemCaptions>
                                        <Items>
                                            <dx:LayoutGroup Caption="Date Range Picker" ColCount="3" GroupBoxDecoration="HeadingLine">
                                                <Items>
                                                    <dx:LayoutItem Caption="Start Date">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxDateEdit ID="deStart" ClientInstanceName="deStart" runat="server">
                                                                    <ClientSideEvents DateChanged="UpdateInfo"></ClientSideEvents>
                                                                    <ValidationSettings Display="Dynamic" SetFocusOnError="True" CausesValidation="True" ErrorDisplayMode="ImageWithTooltip">
                                                                        <RequiredField IsRequired="True" ErrorText="Start date is required"></RequiredField>
                                                                    </ValidationSettings>
                                                                </dx:ASPxDateEdit>
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="End Date">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxDateEdit ID="deEnd" ClientInstanceName="deEnd" runat="server">
                                                                    <DateRangeSettings StartDateEditID="deStart"></DateRangeSettings>
                                                                    <ClientSideEvents DateChanged="UpdateInfo"></ClientSideEvents>
                                                                    <ValidationSettings Display="Dynamic" SetFocusOnError="True" CausesValidation="True" ErrorDisplayMode="ImageWithTooltip">
                                                                        <RequiredField IsRequired="True" ErrorText="End date is required"></RequiredField>
                                                                    </ValidationSettings>
                                                                </dx:ASPxDateEdit>
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="Duration">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxTextBox ID="tbInfo" ClientInstanceName="tbInfo" runat="server" ReadOnly="True" Width="100">
                                                                </dx:ASPxTextBox>
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem ShowCaption="False" ColSpan="3" Height="50">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxValidationSummary ID="ASPxValidationSummary1" runat="server" ClientInstanceName="validationSummary" ShowErrorsInEditors="True">
                                                                </dx:ASPxValidationSummary>
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem ShowCaption="False">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxButton ID="btnSubmit" runat="server" Text="Fetch" UseSubmitBehavior="False"
                                                                    AutoPostBack="false">
                                                                    <ClientSideEvents Click="function(s, e) { btnFetch_Click(); }" />
                                                                </dx:ASPxButton>
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                </Items>
                                            </dx:LayoutGroup>
                                        </Items>
                                    </dx:ASPxFormLayout>

                            </dx:SplitterContentControl>
                        </ContentCollection>
                    </dx:SplitterPane>
                </Panes>
            </dx:SplitterPane>
        </panes>
    </dx:ASPxSplitter>

</asp:Content>
