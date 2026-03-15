<%@ Page Title="DataGrids" Language="C#" MasterPageFile="~/ServerStuff/ServerStuff.master" AutoEventWireup="true" CodeBehind="DataGrid.aspx.cs" Inherits="UFWebClient.HTML5.DataGrid" %>

<%@ Register assembly="DevExpress.Web.v21.2, Version=21.2.13.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server">
    DataGrids
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">

    <script type="text/javascript">
        var Start;
        function OnInit(s, e) {
            AdjustSize();
        }
        function OnBeginCallback(s, e) {
            start = new Date();
            ClientTimeLabel.SetText("Loading...");
        }
        function OnEndCallback(s, e) {
            AdjustSize();
            ClientTimeLabel.SetText(new Date() - start);
        }
        function OnControlsInitialized(s, e) {
            ASPxClientUtils.AttachEventToElement(window, "resize", function(evt) {
                AdjustSize();
            });
        }
        function AdjustSize() {
            var height = Math.max(0, document.documentElement.clientHeight);
            grid.SetHeight(height);
        }
    </script>

    <dx:ASPxSplitter ID="ASPxSplitter1" runat="server" Width="100%" Height="1024px" ResizingMode="Live">
        <panes>
            <dx:SplitterPane Size="20%" Name="listBoxContainer" ShowCollapseBackwardButton="True">
                <ContentCollection>
                    <dx:SplitterContentControl ID="SplitterContentControl1" runat="server" SupportsDisabledAttribute="True">
						<dx:ASPxListBox ID="ASPxListBox1" runat="server" Height="100%" Width="100%" AutoPostBack="true" />
                    </dx:SplitterContentControl>
                </ContentCollection>
            </dx:SplitterPane>
            <dx:SplitterPane ShowCollapseBackwardButton="True" >
                <ContentCollection>
                    <dx:SplitterContentControl ID="SplitterContentControl2" runat="server" SupportsDisabledAttribute="True">

                    <uc:ToolbarExport runat="server" ID="ToolbarExport" ExportItemTypes="Pdf,Xls,Xlsx,Rtf,Csv" OnItemClick="ToolbarExport_ItemClick" />
                        <table class="OptionsBottomMargin">
                            <tr>
                                <td style="padding-right: 4px">
                                    <dx:ASPxButton ID="btnRefresh" runat="server" Text="Refresh" UseSubmitBehavior="False"
                                        AutoPostBack="false">
                                        <ClientSideEvents Click="function(s, e) { grid.Refresh(); }" />
                                    </dx:ASPxButton>
                                </td>
                                <td style="padding-right: 4px">
                                    <dx:ASPxButton ID="btnSelectAll" runat="server" Text="Select All" UseSubmitBehavior="False"
                                        AutoPostBack="false">
                                        <ClientSideEvents Click="function(s, e) { grid.SelectRows(); }" />
                                    </dx:ASPxButton>
                                </td>
                                <td style="padding-right: 4px">
                                    <dx:ASPxButton ID="btnUnselectAll" runat="server" Text="Unselect All" UseSubmitBehavior="False"
                                        AutoPostBack="false">
                                        <ClientSideEvents Click="function(s, e) { grid.UnselectRows(); }" />
                                    </dx:ASPxButton>
                                </td>
                                <td style="padding-right: 4px">
                                    <dx:ASPxButton ID="btnSelectAllOnPage" runat="server" Text="Select all on the page"
                                        UseSubmitBehavior="False" AutoPostBack="false">
                                        <ClientSideEvents Click="function(s, e) { grid.SelectAllRowsOnPage(); }" />
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnUnselectAllOnPage" runat="server" Text="Unselect all on the page"
                                        UseSubmitBehavior="False" AutoPostBack="false">
                                        <ClientSideEvents Click="function(s, e) { grid.UnselectAllRowsOnPage(); }" />
                                    </dx:ASPxButton>
                                </td>
                            </tr>
                        </table>

                        <dx:ASPxLabel runat="server" ID="ASPxLabel4" ClientInstanceName="ClientTimeLabel" Text="" />
						<dx:ASPxGridView ID="GridData" runat="server" ClientInstanceName="grid" AutoGenerateColumns="False" Settings-ShowGroupPanel="true" OnLoad="GridData_Load" Width="100%">
                            <SettingsSearchPanel Visible="true" />
                            <Settings ShowFooter="True" ShowHeaderFilterButton="true" VerticalScrollBarMode="Auto" HorizontalScrollBarMode="Auto" ShowFilterBar="Visible"/>
                            <SettingsPager PageSize="20">
                                <PageSizeItemSettings Visible="true" ShowAllItem="true" />
                            </SettingsPager>                        
                            <SettingsBehavior AllowSelectByRowClick="true" />
                            <ClientSideEvents Init="OnInit" EndCallback="OnEndCallback" BeginCallback="OnBeginCallback"/>
						</dx:ASPxGridView>

                        <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="GridData" ExportSelectedRowsOnly="true" />
                        <dx:ASPxLabel ID="LabelErrorInfo" runat="server" Width="100%" Height="100%" Wrap="True" Font-Bold="true" />
                        <dx:ASPxGlobalEvents ID="ge" runat="server">
                            <ClientSideEvents ControlsInitialized="OnControlsInitialized" />
                        </dx:ASPxGlobalEvents>
                    </dx:SplitterContentControl>
                </ContentCollection>
            </dx:SplitterPane>
        </panes>
    </dx:ASPxSplitter>

</asp:Content>
