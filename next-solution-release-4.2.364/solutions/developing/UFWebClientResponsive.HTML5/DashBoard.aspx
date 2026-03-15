<%@ Page Title="DashBoard" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeBehind="DashBoard.aspx.cs" Inherits="UFWebClient.HTML5.DashBoard" %>

<%@ register assembly="DevExpress.Dashboard.v21.2.Web.WebForms, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.DashboardWeb" tagprefix="dx" %>

<%@ Register assembly="DevExpress.Web.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<%@ Register assembly="DevExpress.Dashboard.v21.2.Web.WebForms, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.DashboardWeb.Designer" tagprefix="dx" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <script type="text/javascript">
        function saveToUrl(key, value) {
            var uri = location.href;
            var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
            var separator = uri.indexOf('?') !== -1 ? "&" : "?";
            var newParameterValue = value ? key + "=" + encodeURIComponent(value) : "";
            var newUrl;
            if (uri.match(re)) {
                var separator = !!newParameterValue ? '$1' : "";
                newUrl = uri.replace(re, separator + newParameterValue + '$2');
            }
            else if (!!newParameterValue) {
                newUrl = uri + separator + newParameterValue;
            }
            if (newUrl) {
                history.replaceState({}, "", newUrl);
            }
        }

        function onCustomizeMenuItems(designer, eventArgs) {
            if(!designer.cpIsSqlExpressInstalled) {
                var itemNew = eventArgs.FindById('new');
                if(itemNew) {
                    itemNew.template = 'dx-dshd-form-new-disallowed';
                }
            }
        }

        function onBeforeRender(s, e) {
            var switchtoview = <%= SwitchToViewer() %>;
            if (switchtoview == true)
            {
                if (s) {
                    var designer = s.GetDashboardControl();
                    saveToUrl("mode", s.GetWorkingMode());
                    designer.isDesignMode.subscribe(function(value) {
                        saveToUrl("mode", s.GetWorkingMode());
                    });
                    var extension = new DevExpress.Dashboard.DashboardPanelExtension(designer);
                    extension.allowSwitchToDesigner(false);
                    designer.registerExtension(extension);
                }
                webDesigner.SwitchToViewer();
            }
        }
    </script>

    <dx:ASPxDashboard ID="ASPxDashboardDesigner1" runat="server" 
        DashboardStorageFolder="~/App_Data" 
        OnConfigureDataConnection="ASPxDashboard1_ConfigureDataConnection"
        ClientInstanceName="webDesigner">
        <ClientSideEvents BeforeRender="onBeforeRender" />
    </dx:ASPxDashboard>
</asp:Content>
