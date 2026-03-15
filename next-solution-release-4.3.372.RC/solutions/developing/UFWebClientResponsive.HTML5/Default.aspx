<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Main.master" CodeBehind="Default.aspx.cs" Inherits="UFWebClientResponsive_HTML5._Default" %>



<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="server">
    
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
    

<%--    <dx:ASPxDockZone ID="zone1" ClientInstanceName="zone1" runat="server" PanelSpacing="3" 
        Width="100%" Height="100%">
    </dx:ASPxDockZone>
    <dx:ASPxDockManager ID="ASPxDockManager" runat="server" SaveStateToCookies="True" 
            SaveStateToCookiesID="panelCookies">
    </dx:ASPxDockManager>
    <asp:PlaceHolder ID="holder" runat="server" />--%>

    <dx:ASPxPanel ID="LeftPanel" runat="server" FixedPosition="WindowLeft" Collapsible="true" ScrollBars="Auto">
        <SettingsAdaptivity CollapseAtWindowInnerWidth="900" />
        <PanelCollection>
            <dx:PanelContent runat="server" SupportsDisabledAttribute="True">
                <dx:ASPxTreeView ID="ASPxTreeView" runat="server">

                </dx:ASPxTreeView>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxPanel>
<%--    <dx:ASPxPanel ID="LeftPanel" runat="server" FixedPosition="WindowLeft" Collapsible="false" ScrollBars="Auto">
        <PanelCollection>
            <dx:PanelContent runat="server" SupportsDisabledAttribute="True">
                <dx:ASPxNavBar ID="nbMain" runat="server">
                <Paddings Padding="0px" />
                </dx:ASPxNavBar>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxPanel>--%>
    <dx:ASPxImage ID="ASPxImage1" runat="server" ImageUrl="~/Images/SPLASHNEXT--webclient.png" ShowLoadingImage="True">
    </dx:ASPxImage>
</asp:Content>