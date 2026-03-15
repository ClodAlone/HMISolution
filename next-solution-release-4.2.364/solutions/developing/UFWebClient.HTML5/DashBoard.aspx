<%@ Page Title="DashBoard" Language="C#" MasterPageFile="~/ServerStuff/ServerStuff.master" AutoEventWireup="true" CodeBehind="DashBoard.aspx.cs" Inherits="UFWebClient.HTML5.DashBoard" %>

<%@ register assembly="DevExpress.Dashboard.v21.2.Web.WebForms, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.DashboardWeb" tagprefix="dx" %>

<%@ Register assembly="DevExpress.Web.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<%@ Register assembly="DevExpress.Dashboard.v21.2.Web.WebForms, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.DashboardWeb.Designer" tagprefix="dx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server">
    DashBoard
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">
    <dx:ASPxDashboard ID="ASPxDashboardDesigner1" runat="server" ClientInstanceName="webDesigner">
    </dx:ASPxDashboard>

    <script type="text/javascript">

        var switchtoview = <%= SwitchToViewer() %>;
        if (switchtoview == true)
            webDesigner.SwitchToViewer();
    </script>

</asp:Content>
